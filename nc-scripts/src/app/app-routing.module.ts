import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { TasksComponent } from './tasks/tasks.component';
import { TaskDetailsComponent } from './taskdetails/taskdetails.component';
import { ScriptsComponent } from './scripts/scripts.component';
import { ScriptDetailsComponent } from './scriptdetails/scriptdetails.component';
import { WorkflowsComponent } from './workflows/workflows.component';
import { WorkflowDetailsComponent } from './workflow-details/workflow-details.component';
import { AuthGuard } from './guards/auth.guard';
import { LoginComponent } from './login/login.component';
import { ScheduledTasksComponent } from './scheduled-tasks/scheduled-tasks.component';
import { ScheduledTaskEditorComponent } from './scheduled-task-editor/scheduled-task-editor.component';

const routes: Routes = [
  {path:'', redirectTo: '/tasks', pathMatch: 'full'},
  {path:'tasks', component: TasksComponent, canActivate: [AuthGuard]},
  {path:'tasks/:taskId', component: TaskDetailsComponent, canActivate: [AuthGuard]},
  {path:'scripts', component: ScriptsComponent, canActivate: [AuthGuard]},
  {path:'scripts/:scriptId', component: ScriptDetailsComponent, canActivate: [AuthGuard]},
  {path:'scripts/create', component: ScriptDetailsComponent, canActivate: [AuthGuard]},
  {path:'workflows', component: WorkflowsComponent, canActivate: [AuthGuard]},
  {path:'workflows/:workflowId', component: WorkflowDetailsComponent, canActivate: [AuthGuard]},
  {path:'workflows/create', component: WorkflowDetailsComponent, canActivate: [AuthGuard]},
  {path:'scheduler', component: ScheduledTasksComponent, canActivate: [AuthGuard]},
  {path:'scheduler/:taskId', component: ScheduledTaskEditorComponent, canActivate: [AuthGuard]},
  {path:'scheduler/create', component: ScheduledTaskEditorComponent, canActivate: [AuthGuard]},
  {path:'login', component: LoginComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
