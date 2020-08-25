import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { TasksComponent } from './tasks/tasks.component';
import { TaskDetailsComponent } from './taskdetails/taskdetails.component';
import { ScriptsComponent } from './scripts/scripts.component';
import { ScriptDetailsComponent } from './scriptdetails/scriptdetails.component';
import { WorkflowsComponent } from './workflows/workflows.component';
import { WorkflowDetailsComponent } from './workflow-details/workflow-details.component';

const routes: Routes = [
  {path:'', redirectTo: '/tasks', pathMatch: 'full'},
  {path:'tasks', component: TasksComponent},
  {path:'tasks/:taskId', component: TaskDetailsComponent},
  {path:'scripts', component: ScriptsComponent},
  {path:'scripts/:scriptId', component: ScriptDetailsComponent},
  {path:'scripts/create', component: ScriptDetailsComponent},
  {path:'workflows', component: WorkflowsComponent},
  {path:'workflows/:workflowId', component: WorkflowDetailsComponent},
  {path:'workflows/create', component: WorkflowDetailsComponent},
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
