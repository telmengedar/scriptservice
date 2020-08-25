import { Component, OnInit, OnDestroy } from '@angular/core';
import { WorkflowService } from '../services/workflow.service';
import { Workflow } from '../dto/workflows/workflow';
import { timer, Subscription } from 'rxjs';
import { Router } from '@angular/router';

/**
 * provides a list of workflows.
 * Allows to create new workflows, edit existing and to delete workflows.
 */
@Component({
  selector: 'app-workflows',
  templateUrl: './workflows.component.html',
  styleUrls: ['./workflows.component.css']
})
export class WorkflowsComponent implements OnInit, OnDestroy {
  workflows: Workflow[];
  reloadsub: Subscription;
  page: number;
  pages: number;

  constructor(private workflowservice: WorkflowService, private router: Router) { }

  ngOnInit() {
    this.listWorkflows();
    this.reloadsub=timer(1000, 1000).subscribe(t=>this.listWorkflows());
  }

  ngOnDestroy() {
    this.reloadsub.unsubscribe();
  }

  /**
   * reloads list of workflows with currently set filter
   */
  listWorkflows() {
    this.workflowservice.listWorkflows().subscribe(p=>{
      this.workflows=p.result;
      this.pages=Math.ceil(p.total/50);
      if(p.continue) {
        this.page=Math.floor(p.continue/50)
      }
      else this.page=this.pages;
    });
  }

    /**
   * deletes a workflow
   * @param workflowId id of workflow to delete
   */
  deleteWorkflow(workflowId: number): void {
    this.workflowservice.deleteWorkflow(workflowId).subscribe(s=>{
      this.listWorkflows();
    });
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

}
