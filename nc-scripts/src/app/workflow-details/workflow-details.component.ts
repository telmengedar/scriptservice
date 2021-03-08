import {Location} from '@angular/common';
import { Component, ViewChild, HostListener, OnInit } from '@angular/core';
import { WorkflowDetails } from '../dto/workflows/workflowdetails';
import { Node, Edge, ClusterNode, NodeDimension } from '@swimlane/ngx-graph';
import { WorkflowService } from '../services/workflow.service';
import { ActivatedRoute } from '@angular/router';
import { Subject } from 'rxjs';
import { NodeData } from '../dto/workflows/nodeData';
import { IndexTransition } from '../dto/workflows/indexTransition';
import { MatMenuTrigger, MatSnackBar } from '@angular/material';
import { v4 as guid } from 'uuid';
import {MatDialog} from '@angular/material/dialog';
import { TransitionEditorComponent } from './transition-editor/transition-editor.component';
import { Transition } from '../dto/workflows/transition';
import { NodeEditorComponent } from './node-editor/node-editor.component';
import { WorkflowStructure } from '../dto/workflows/workflowstructure';
import { WorkflowNode } from '../dto/workflows/workflownode';
import { NodeEditorParameters } from '../dto/workflows/nodeeditorparameters';
import { ImportDeclaration } from '../dto/workflows/importdeclaration';
import { TransitionType } from '../dto/workflows/transitionType';
import { NodeType } from '../dto/workflows/nodetype';
import { TestWorkableComponent } from '../dialogs/test-workable/test-workable.component';
import { WorkableType } from '../dto/tasks/workabletype';

@Component({
  selector: 'app-workflow-details',
  templateUrl: './workflow-details.component.html',
  styleUrls: ['./workflow-details.component.css']
})
export class WorkflowDetailsComponent implements OnInit{
  NodeType=NodeType;
  workflowid?: number;

  workflow: WorkflowDetails={
    id: 0,
    revision: 0,
    name: "",
    scope: "",
    nodes: [],
    transitions: []
  };
  changed: boolean=false;

  nodes: Node[]=[];
  transitions: Edge[]=[];
  clusters: ClusterNode[];

  updateGraph$: Subject<boolean>=new Subject();

  selectedNode: Node;
  selectedEdge: Edge;

  @ViewChild(MatMenuTrigger)
  contextMenu: MatMenuTrigger;
  contextMenuPosition = { x: '0px', y: '0px' };

  constructor(private workflowservice: WorkflowService, private location: Location, route: ActivatedRoute, private dialogservice: MatDialog, private snackbar: MatSnackBar) { 
    if(route.snapshot.params.workflowId!=="create") {
        this.workflowid=route.snapshot.params.workflowId;
    }
  }

  ngOnInit() {
    this.createNode({
      id: guid(),
      name: "Start",
      type: NodeType.Start,
      parameters: {}
    });

    if(this.workflowid)
      this.workflowservice.getWorkflowById(this.workflowid).subscribe(w=>{
        this.processWorkflowData(w);
      });
  }

  loadRevision(): void {
    if(!this.workflowid)
      return;
    
    if(this.workflow.revision<=0)
    {
      this.workflowservice.getWorkflowById(this.workflowid)
      .toPromise()
      .then(w=>{
        this.processWorkflowData(w);
        this.changed=false;
      })
      .catch(e=>{
        this.snackbar.open(e.error.text, "Close");
      });
      return;
    }

    this.workflowservice.getWorkflowRevision(this.workflow.id, this.workflow.revision)
    .toPromise()
    .then(w=>{
      this.processWorkflowData(w);
      this.changed=false;
    })
    .catch(e=>{
      this.snackbar.open(e.error.text, "Close");
    });
  }

  private processWorkflowData(data: WorkflowDetails): void {
    this.workflow=data;
    this.nodes=[];
    this.transitions=[];
    data.nodes.forEach(n=>{
      this.createNode(n);
    });
    data.transitions.forEach(t=>{
      this.createTransition(t);
    });
    this.generateClusters();
  }

  layoutNode(data: WorkflowNode, node: Node): void {
    const threshold=80+20*Math.max(data.name.length*10, 100)/100;
    let linecount=1;
    let maxsize=threshold;
    let currentsize=0;
    let currentline="";
    node.data.lines=[];
    for(let character of data.name) {
      switch(character) {
        case ' ':
          if(currentsize>threshold) {
            maxsize=Math.max(maxsize, currentsize);
            currentsize=0;
            ++linecount;
            node.data.lines.push(currentline);
            currentline="";
          }
          else {
            currentsize+=10;
            currentline+=' ';
          }
        default:
          currentline+=character;
          currentsize+=10;
          break;
      }
    }

    if(currentsize>threshold)
      maxsize=Math.max(maxsize, currentsize);

    if(currentline!=="")
      node.data.lines.push(currentline);
      
    node.label=data.name;
    node.dimension={
      width: maxsize,
      height: 15+Math.max(65,50+linecount*15)+(data.variable?15:0)
    }
  }

  private createNode(node: WorkflowNode): void {
    let graphnode: Node={
      id: node.id,
      label: node.name,
      data: {
        node: node,
        type: node.type,
        parameters: node.parameters,
        highlighted: false
      }
    };
    this.layoutNode(node, graphnode);
    this.nodes.push(graphnode);
  }

  private createTransition(transition: Transition): void {
    this.transitions.push({
      source: transition.originId,
      target: transition.targetId,
      label: transition.condition,
      data: {
        transition: transition,
        highlighted: false
      }
    });
  }

  /**
   * opens a dialog used to edit a transition
   * @param edge edge representing transition to edit
   */
  editTransition(event: MouseEvent, edge: Edge): void {
    event.stopPropagation();
    if(this.selectedEdge)
    {
      this.selectedEdge.data.highlighted=false;
      this.selectedEdge=null;
    }

    let transitiondialog=this.dialogservice.open(TransitionEditorComponent, {
      data: edge.data.transition,
      width: '50%'
    });
    transitiondialog.afterClosed().subscribe(r=>{
      this.changed=true;
    });
  }

  private generateClusters(): void {
    let clusters:ClusterNode[]=[];

    this.nodes.forEach(n=>{
      if(!n.data.node.group)
        return;
      
      var cluster:ClusterNode=clusters.find(c=>c.id===n.data.node.group);
      if(!cluster)
      {
        cluster={
          id: n.data.node.group,
          label: n.data.node.group,
          childNodeIds: []
        };
        clusters.push(cluster);
      }
      cluster.childNodeIds.push(n.data.node.id);
    });

    this.clusters=clusters;
  }

  private determineImports(): ImportDeclaration[] {
    let startnode=this.nodes.find(n=>NodeType.getNodeTypeValue(n.data.type)===NodeType.Start);
    if(!startnode)
      return [];
    
    return startnode.data.parameters.imports;
  }

  /**
   * opens a dialog used to edit a node
   * @param event mouse event which triggered this method
   * @param node node to edit
   */
  editNode(event: MouseEvent, node: Node) {
    event.stopPropagation();

    if(event.shiftKey) {
      let nodeid: string=this.addNode();
      this.createTransition({
        originId: node.id,
        targetId: nodeid,
        type: TransitionType.Standard
      });
      this.refreshLayout();
    } else {
      let nodedialog=this.dialogservice.open<NodeEditorComponent, NodeEditorParameters>(NodeEditorComponent, {
        data: {
          node: node.data.node,
          imports: this.determineImports(),
        },
        panelClass: 'editor-dialog-container',
        width: '50%'
      });
      nodedialog.afterClosed().subscribe(n=>{
        this.changed=true;
        this.generateClusters();
      });
    }
  }

  /**
   * navigates back to the script list page
   */
  back(): void {
    this.location.back();
  }

  /**
   * starts test for the current workflow
   */
  startTest(): void {
    this.dialogservice.open(TestWorkableComponent, {
      data: {
        type: WorkableType.Workflow,
        workable: {
          scope: this.workflow.scope,
          name: this.workflow.name,
          nodes: this.buildNodes(),
          transitions: this.buildTransitions()
        }
      },
      width: '50%'
    });
  }

  private buildNodes(): NodeData[] {
    let result: NodeData[]=[];
    this.nodes.forEach(n=>result.push(n.data.node));

    return result;
  }

  private getNodeIndex(id: string): number {
    return this.nodes.findIndex(n=>n.id===id);
  }

  private buildTransitions(): IndexTransition[] {
    let result: IndexTransition[]=[];
    this.transitions.forEach(t=>{
      result.push({
        originIndex: this.getNodeIndex(t.source),
        targetIndex: this.getNodeIndex(t.target),
        condition: t.data.transition.condition,
        type: t.data.transition.type,
        log: t.data.transition.log
      });
    });
    return result;
  }

  onContextMenu(event: MouseEvent) {
    event.preventDefault();
    this.contextMenuPosition.x = event.clientX + 'px';
    this.contextMenuPosition.y = event.clientY + 'px';
    //this.contextMenu.menuData = { 'item': item };
    this.contextMenu.menu.focusFirstItem('mouse');
    this.contextMenu.openMenu();
  }

  addNode(): string {
    let n: NodeData={
      name: "New Node",
      type: NodeType.Node,
      parameters: {}
    }

    let node: Node={
      id: guid(),
      label: n.name,
      dimension: {
        width: 100,
        height: 80
      },
      data: {
        node: n,
        type: n.type,
        parameters: n.parameters
      }
    };
    this.nodes.push(node);
    this.changed=true;
    return node.id;
  }

  refreshLayout(): void {
    this.updateGraph$.next(true);
  }

  resetTransition(): void {
    if(this.selectedNode) {
      this.selectedNode.data.highlighted=false;
      this.selectedNode=null;
    }

    if(this.selectedEdge) {
      this.selectedEdge.data.highlighted=false;
      this.selectedEdge=null;  
    }
  }

  edgeClick(event: MouseEvent, edge: Edge): void {
    if(this.selectedNode)
      this.selectedNode.data.highlighted=false;
    
    if(this.selectedEdge) {
      if(this.selectedEdge!==edge)
        this.selectedEdge.data.highlighted=false;
    }

    this.selectedEdge=edge;
    this.selectedEdge.data.highlighted=true;
    event.stopPropagation();
  }

  nodeClick(event: MouseEvent, node: Node): void {
    if(!event.ctrlKey)
      return;
    
    if(this.selectedEdge)
      this.selectedEdge.data.highlighted=false;
    
    if(!this.selectedNode) {
      this.selectedNode=node;
      node.data.highlighted=true;
      event.stopPropagation();
      return;
    }

    if(this.selectedNode) {
      if(node!==this.selectedNode) {
        if(this.transitions.findIndex(t=>t.source===this.selectedNode.id && t.target===node.id) < 0) {          
          this.createTransition({
            originId: this.selectedNode.id,
            targetId: node.id,
            type: TransitionType.Standard
          });
          this.changed=true;
          this.refreshLayout();
        }
      }
      this.selectedNode.data.highlighted=false;
      this.selectedNode=null;
    }    
  }

  /**
   * saves changes
   */
  save(): void {
    let data: WorkflowStructure={
      scope: this.workflow.scope,
      name: this.workflow.name,
      nodes: this.buildNodes(),
      transitions: this.buildTransitions()
    };

    if(this.workflowid)
      this.workflowservice.updateWorkflow(this.workflowid, data).subscribe(w=>this.processWorkflowData(w));
    else this.workflowservice.createWorkflow(data).subscribe(w=>{
      this.workflowid=w.id;
      this.processWorkflowData(w);
    });
    this.changed=false;
  }

  @HostListener('document:keyup', ['$event'])
  handleKey(event: KeyboardEvent): void {
    if(event.key==="Delete") {
      if(this.selectedEdge) {
        const index: number=this.transitions.findIndex(t=>t.source===this.selectedEdge.source && t.target===this.selectedEdge.target);
        if(index>-1)
          this.transitions.splice(index, 1);
        this.selectedEdge=null;
        this.changed=true;
        this.refreshLayout();
      }
      else if(this.selectedNode) {
        let index: number=this.nodes.findIndex(n=>n.id===this.selectedNode.id);
        if(index>-1)
          this.nodes.splice(index, 1);

        do {
          index=this.transitions.findIndex(t=>t.source==this.selectedNode.id || t.target==this.selectedNode.id);
          if(index>-1)
            this.transitions.splice(index, 1);
        } while(index>-1);
        this.changed=true;
        this.selectedNode=null;
        this.refreshLayout();
      }
    }
  }
}
