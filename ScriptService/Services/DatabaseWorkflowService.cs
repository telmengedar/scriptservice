using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NightlyCode.AspNetCore.Services.Data;
using NightlyCode.AspNetCore.Services.Errors.Exceptions;
using NightlyCode.Database.Clients;
using NightlyCode.Database.Entities;
using NightlyCode.Database.Entities.Operations;
using NightlyCode.Database.Entities.Operations.Prepared;
using NightlyCode.Database.Fields;
using ScriptService.Dto;
using ScriptService.Dto.Patches;
using ScriptService.Dto.Workflows;
using ScriptService.Extensions;

namespace ScriptService.Services {

    /// <summary>
    /// workflow service storing data in database
    /// </summary>
    public class DatabaseWorkflowService : IWorkflowService {
        readonly IEntityManager database;
        readonly IArchiveService archiveservice;

        readonly PreparedOperation insertworkflow;
        readonly PreparedOperation deletetransitions;
        readonly PreparedOperation deletenodes;
        readonly PreparedOperation deleteworkflow;

        readonly PreparedLoadOperation<Workflow> loadworkflowbyid;
        readonly PreparedLoadOperation<Workflow> loadworkflowbyname;
        readonly PreparedLoadOperation loadtransitions;
        readonly PreparedLoadOperation loadnodes;

        readonly PreparedOperation insertnode;
        readonly PreparedOperation inserttransition;
        readonly PreparedOperation updateworkflow;

        /// <summary>
        /// creates a new <see cref="DatabaseWorkflowService"/>
        /// </summary>
        /// <param name="database">access to database</param>
        /// <param name="archiveservice">access to data archives</param>
        public DatabaseWorkflowService(IEntityManager database, IArchiveService archiveservice) {
            this.database = database;
            this.archiveservice = archiveservice;
            database.UpdateSchema<Workflow>();
            database.UpdateSchema<WorkflowNode>();
            database.UpdateSchema<WorkflowTransition>();

            insertworkflow = database.Insert<Workflow>().Columns(w=>w.Revision, w => w.Name).ReturnID().Prepare();
            updateworkflow = database.Update<Workflow>().Set(w => w.Revision == w.Revision + 1, w => w.Name == DBParameter.String).Where(w => w.Id == DBParameter.Int64).Prepare();

            deletetransitions = database.Delete<WorkflowTransition>().Where(t => t.WorkflowId == DBParameter.Int64).Prepare();
            deletenodes = database.Delete<WorkflowNode>().Where(n => n.WorkflowId == DBParameter.Int64).Prepare();
            deleteworkflow = database.Delete<Workflow>().Where(w => w.Id == DBParameter.Int64).Prepare();

            loadworkflowbyid = database.Load<Workflow>().Where(w => w.Id == DBParameter.Int64).Prepare();
            loadworkflowbyname = database.Load<Workflow>().Where(w => w.Name == DBParameter.String).Prepare();

            loadnodes = database.Load<WorkflowNode>(n => n.Id, n => n.Name, n=>n.Group, n => n.Type, n => n.Parameters, n=>n.Variable).Where(n => n.WorkflowId == DBParameter.Int64).Prepare();
            loadtransitions = database.Load<WorkflowTransition>(t => t.OriginId, t => t.TargetId, t => t.Condition, t=>t.Type, t=>t.Log).Where(t => t.WorkflowId == DBParameter.Int64).Prepare();
            insertnode = database.Insert<WorkflowNode>().Columns(n => n.Id, n => n.WorkflowId, n => n.Name, n => n.Type, n => n.Parameters, n=>n.Group, n=>n.Variable).Prepare();
            inserttransition = database.Insert<WorkflowTransition>().Columns(t => t.WorkflowId, t => t.OriginId, t => t.TargetId, t => t.Condition, t=>t.Type, t=>t.Log).Prepare();
        }

        Task<Workflow> LoadWorkflowByName(string name) {
            return loadworkflowbyname.ExecuteEntityAsync(name);
        }

        Task UpdateWorkflow(Transaction transaction, long workflowid, string name) {
            return updateworkflow.ExecuteAsync(transaction, name, workflowid);
        }

        Task CreateNode(Transaction transaction, Guid nodeid, long workflowid, string name, string group, NodeType type, object parameters, string result) {
            return insertnode.ExecuteAsync(transaction, nodeid, workflowid, name, type, parameters.Serialize(), group, result);
        }

        Task CreateTransition(Transaction transaction, long workflowid, Guid origin, Guid target, string condition, TransitionType type, string log) {
            return inserttransition.ExecuteAsync(transaction, workflowid, origin, target, condition, type, log);
        }

        Task DeleteTransitions(Transaction transaction, long workflowid) {
            return deletetransitions.ExecuteAsync(transaction, workflowid);
        }

        Task DeleteNodes(Transaction transaction, long workflowid) {
            return deletenodes.ExecuteAsync(transaction, workflowid);
        }

        async Task<WorkflowDetails> FillWorkflow(WorkflowDetails workflow) {
            workflow.Nodes = await loadnodes.ExecuteTypeAsync(r => new NodeDetails {
                Id = r.GetValue<Guid>(0),
                Name = r.GetValue<string>(1),
                Group = r.GetValue<string>(2),
                Type = r.GetValue<NodeType>(3),
                Parameters = r.GetValue<string>(4).Deserialize<IDictionary<string,object>>(),
                Variable = r.GetValue<string>(5)
            }, workflow.Id);
            workflow.Transitions = await loadtransitions.ExecuteTypeAsync(r => new TransitionData {
                OriginId = r.GetValue<Guid>(0),
                TargetId = r.GetValue<Guid>(1),
                Condition = r.GetValue<string>(2),
                Type= r.GetValue<TransitionType>(3),
                Log=r.GetValue<string>(4)
            }, workflow.Id);
            return workflow;
        }

        /// <inheritdoc />
        public async Task<long> CreateWorkflow(WorkflowStructure data) {
            if (data.Transitions != null) {
                int nodecount = data.Nodes?.Length ?? 0;
                foreach (IndexTransition transition in data.Transitions) {
                    if (transition.OriginIndex < 0 || transition.OriginIndex >= nodecount)
                        throw new IndexOutOfRangeException("Transition origin index out of range");
                    if (transition.OriginIndex < 0 || transition.TargetIndex < 0 || transition.OriginIndex >= nodecount || transition.TargetIndex >= nodecount)
                        throw new IndexOutOfRangeException("Transition target index out of range");
                    if(transition.OriginIndex==transition.TargetIndex)
                        throw new InvalidOperationException("Transition target must not be the same node as origin");
                }
            }

            using Transaction transaction = database.Transaction();

            long workflowid = await insertworkflow.ExecuteAsync(1, data.Name);
            await CreateNodeAndTransitions(transaction, workflowid, data.Nodes, data.Transitions);
            transaction.Commit();
            return workflowid;
        }

        /// <inheritdoc />
        public async Task UpdateWorkflow(long workflowid, WorkflowStructure data) {
            if(data.Transitions != null) {
                int nodecount = data.Nodes?.Length ?? 0;
                foreach(IndexTransition transition in data.Transitions) {
                    if(transition.OriginIndex < 0 || transition.OriginIndex >= nodecount)
                        throw new IndexOutOfRangeException("Transition origin index out of range");
                    if(transition.OriginIndex < 0 || transition.TargetIndex < 0 || transition.OriginIndex >= nodecount || transition.TargetIndex >= nodecount)
                        throw new IndexOutOfRangeException("Transition target index out of range");
                }
            }

            using Transaction transaction = database.Transaction();
            await ArchiveWorkflow(transaction, workflowid);
            
            await DeleteNodes(transaction, workflowid);
            await DeleteTransitions(transaction, workflowid);
            await UpdateWorkflow(transaction, workflowid, data.Name);
            await CreateNodeAndTransitions(transaction, workflowid, data.Nodes, data.Transitions);
            transaction.Commit();
        }

        async Task CreateNodeAndTransitions(Transaction transaction, long workflowid, NodeData[] nodes, IndexTransition[] transitions) {
            if(nodes != null) {
                List<Guid> nodeids = new List<Guid>();
                foreach(NodeData node in nodes) {
                    Guid nodeid = Guid.NewGuid();
                    await CreateNode(transaction, nodeid, workflowid, node.Name, node.Group, node.Type, node.Parameters, node.Variable);
                    nodeids.Add(nodeid);
                }

                if(transitions != null) {
                    foreach(IndexTransition transition in transitions)
                        await CreateTransition(transaction, workflowid, nodeids[transition.OriginIndex], nodeids[transition.TargetIndex], transition.Condition, transition.Type, transition.Log);
                }
            }
        }

        /// <inheritdoc />
        public async Task<WorkflowDetails> GetWorkflow(long workflowid, int? revision=null) {
            Workflow workflow = await loadworkflowbyid.ExecuteEntityAsync(workflowid);
            if (workflow == null)
                throw new NotFoundException(typeof(Workflow), workflowid);

            if (revision.HasValue && workflow.Revision != revision.Value) {
                if(revision.Value != workflow.Revision)
                    return await archiveservice.GetArchivedObject<WorkflowDetails>(workflowid, revision.Value, ArchiveTypes.Workflow);
            }

            return await FillWorkflow(new WorkflowDetails {
                Id = workflow.Id,
                Revision = workflow.Revision,
                Name = workflow.Name
            });
        }
        
        /// <inheritdoc />
        public async Task<WorkflowDetails> GetWorkflow(string name, int? revision=null) {
            Workflow workflow = await LoadWorkflowByName(name);
            if(workflow == null)
                throw new NotFoundException(typeof(Workflow), name);

            if (revision.HasValue && workflow.Revision != revision.Value) {
                if(revision.Value!=workflow.Revision)
                    return await archiveservice.GetArchivedObject<WorkflowDetails>(workflow.Id, revision.Value, ArchiveTypes.Workflow);
            }

            return await FillWorkflow(new WorkflowDetails {
                Id = workflow.Id,
                Revision = workflow.Revision,
                Name = workflow.Name
            });
        }

        /// <inheritdoc />
        public async Task<Page<Workflow>> ListWorkflows(ListFilter filter = null) {
            filter??=new ListFilter();
            LoadOperation<Workflow> operation = database.Load<Workflow>();


            return Page<Workflow>.Create(
                await operation.ApplyFilter(filter).ExecuteEntitiesAsync(),
                await database.Load<Workflow>(w => DBFunction.Count()).ExecuteScalarAsync<long>(),
                filter.Continue
            );
        }

        IEnumerable<T> GetItems<T>(object value) {
            if (value is IDictionary<string, object> dic) {
                yield return dic.Deserialize<T>();
                yield break;
            }

            if (value is List<object> array) {
                foreach (object item in array) {
                    if (item is IDictionary<string, object> itemdic)
                        yield return itemdic.Deserialize<T>();
                    else
                        throw new ArgumentException($"Patch Array item value type '{item?.GetType()}' not supported");
                }
            }

            throw new ArgumentException($"Patch value type '{value?.GetType()}' not supported");
        }

        /// <inheritdoc />
        public async Task PatchWorkflow(long workflowid, PatchOperation[] patches) {
            if (patches.Length == 0)
                throw new ArgumentException("Patching without patches is invalid", nameof(patches));

            using Transaction transaction = database.Transaction();
            await ArchiveWorkflow(transaction, workflowid);

            List<PatchOperation> nodepatches = new List<PatchOperation>(patches.Where(p => p.Path == "/nodes"));
            List<PatchOperation> transitionpatches = new List<PatchOperation>(patches.Where(p => p.Path == "/transitions"));
            List<PatchOperation> workflowpatches = new List<PatchOperation>(patches.Except(nodepatches).Except(transitionpatches));

            if (workflowpatches.Count > 0)
                await database.Update<Workflow>().Set(w => w.Revision == w.Revision + 1).Where(w => w.Id == workflowid).Patch(workflowpatches.ToArray()).ExecuteAsync(transaction);

            foreach (PatchOperation patch in nodepatches) {
                switch (patch.Op) {
                case PatchOp.Replace:
                    await deletenodes.ExecuteAsync(transaction, workflowid);
                    foreach (NodeDetails node in GetItems<NodeDetails>(patch.Value)) {
                        if (node.Id == Guid.Empty)
                            node.Id = Guid.NewGuid();
                        await CreateNode(transaction, node.Id, workflowid, node.Name, node.Group, node.Type, node.Parameters, node.Variable);
                    }
                    break;
                default:
                    throw new ArgumentException($"Patch operation '{patch.Op}' not supported", nameof(patch.Op));
                }
            }

            foreach (PatchOperation patch in transitionpatches) {
                switch (patch.Op) {
                case PatchOp.Replace:
                    await deletetransitions.ExecuteAsync(transaction, workflowid);
                    foreach (TransitionData transition in GetItems<TransitionData>(patch.Value)) {
                        await CreateTransition(transaction, workflowid, transition.OriginId, transition.TargetId, transition.Condition, transition.Type, transition.Log);
                    }
                    break;
                default:
                    throw new ArgumentException($"Patch operation '{patch.Op}' not supported", nameof(patch.Op));
                }
            }

            transaction.Commit();
        }

        async Task ArchiveWorkflow(Transaction transaction, long workflowid) {
            WorkflowDetails old = await GetWorkflow(workflowid);
            await archiveservice.ArchiveObject(transaction, workflowid, old.Revision, old, ArchiveTypes.Workflow);
        }

        /// <inheritdoc />
        public async Task DeleteWorkflow(long workflowid) {
            using Transaction transaction = database.Transaction();
            await ArchiveWorkflow(transaction, workflowid);

            
            await DeleteTransitions(transaction, workflowid);
            await DeleteNodes(transaction, workflowid);
            await deleteworkflow.ExecuteAsync(transaction, workflowid);
            transaction.Commit();
        }
    }
}