import { Component, OnInit } from '@angular/core';
import { WorkflowService } from '../services/workflow.service';
import { Workflow } from '../dto/workflows/workflow';
import { Router } from '@angular/router';
import { Paging } from '../helpers/paging';
import { Tables } from '../helpers/tables';
import { MatDialog, MatSnackBar, MatTableDataSource } from '@angular/material';
import { Column } from '../dto/column';
import { ConfirmDeleteComponent } from '../dialogs/confirm-delete/confirm-delete.component';
import { Workflows } from '../helpers/workflows';
import { ListFilter } from '../dto/listfilter';
import { Errors } from '../helpers/errors';

/**
 * provides a list of workflows.
 * Allows to create new workflows, edit existing and to delete workflows.
 */
@Component({
  selector: 'app-workflows',
  templateUrl: './workflows.component.html',
  styleUrls: ['./workflows.component.css']
})
export class WorkflowsComponent implements OnInit {
  Paging=Paging;
  Tables=Tables;

  workflows: MatTableDataSource<Workflow>=new MatTableDataSource<Workflow>();
  filter: ListFilter={
    count: Paging.pageSizes[0]
  }

  page: number;
  total: number;

  columns: Column[]=[
    {
      display: "Name",
      name: "name",
    },
    {
      display: "Revision",
      name: "revision",
    },
    {
      display: "",
      name: "_actions",
    }
  ]

  constructor(private workflowservice: WorkflowService, private router: Router, private dialog: MatDialog, private snackbar: MatSnackBar) { }

  ngOnInit() {
    this.listWorkflows();
  }

  /**
   * reloads list of workflows with currently set filter
   */
  listWorkflows() {
    this.workflowservice.listWorkflows(this.filter)
    .toPromise()
    .then(p=>{
      this.workflows.data=p.result;
      this.total=p.total;
      if(p.continue)
        this.page=Math.floor(p.continue/this.filter.count)-1;
      else this.page=p.total/this.filter.count-1;
    })
    .catch(e=>{
      this.snackbar.open(Errors.getErrorText(e));
    });
  }

  /**
   * deletes a workflow
   * @param workflowId id of workflow to delete
   */
  deleteWorkflow(workflow: Workflow): void {
    const dialogRef=this.dialog.open(ConfirmDeleteComponent, {
      data: workflow
    });

    dialogRef.afterClosed().subscribe(r=>{
      if(r)
      {
        this.workflowservice.deleteWorkflow(workflow.id).subscribe(s=>{
          this.listWorkflows();
        });
      }
    });
  }

  /**
   * clones a workflow
   * @param workflowId id of workflow to clone
   */
  cloneWorkflow(workflowId: number): void {
    this.workflowservice.getWorkflowById(workflowId)
    .toPromise()
    .then(w=>{
      let structure=Workflows.toStructure(w);
      structure.name+=" (Clone)";
      this.workflowservice.createWorkflow(structure)
      .toPromise()
      .then(()=>{
        this.listWorkflows();
      })
      .catch(e=>{
        this.snackbar.open(Errors.getErrorText(e));
      })
    })
    .catch(e=>{
      this.snackbar.open(Errors.getErrorText(e));
    })
  }

  /**
   * changes to the page used to create a new workflow
   */
  newWorkflow(): void {
    this.router.navigateByUrl(`/workflows/create`);
  }

  /**
   * changes to the page used to edit a workflow
   * @param workflowId id of workflow to edit
   */
  editWorkflow(workflowId: number): void {
    this.router.navigateByUrl(`/workflows/${workflowId}`);
  }

  onPageChanged(page: number, size: number): void {
    this.filter.continue=page*size;
    this.filter.count=size;
    this.listWorkflows();
  }
}
