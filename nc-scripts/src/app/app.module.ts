import { ReactiveFormsModule } from '@angular/forms'
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule, HTTP_INTERCEPTORS} from '@angular/common/http';
import {CommonModule} from "@angular/common"

import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppRoutingModule } from './app-routing.module';

import { MatIconModule, MatSidenavModule, MatListModule, MatButtonModule, MatNativeDateModule, MatMenuModule, MatDialogModule, MatTableModule, MatPaginatorModule, MAT_SNACK_BAR_DEFAULT_OPTIONS, MatSnackBarModule} from  '@angular/material';
import {MatToolbarModule} from '@angular/material/toolbar';
import {MatGridListModule} from '@angular/material/grid-list';
import {MatInputModule} from '@angular/material/input';
import {MatCheckboxModule} from '@angular/material/checkbox';
import {MatDatepickerModule} from '@angular/material/datepicker';
import {MatSelectModule} from '@angular/material/select';

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { TasksComponent } from './tasks/tasks.component';
import { TaskDetailsComponent } from './taskdetails/taskdetails.component';
import { ScriptsComponent } from './scripts/scripts.component';
import { ScriptDetailsComponent } from './scriptdetails/scriptdetails.component';

import { NgxGraphModule } from '@swimlane/ngx-graph';
import {NgxChartsModule} from '@swimlane/ngx-charts';
import { NgxSelectModule } from 'ngx-select-ex';

import {TooltipModule} from "ngx-tooltip";

import { MonacoEditorModule } from 'ngx-monaco-editor';
import { FormsModule } from '@angular/forms';
import { WorkflowsComponent } from './workflows/workflows.component';
import { WorkflowDetailsComponent } from './workflow-details/workflow-details.component';
import { TransitionEditorComponent } from './workflow-details/transition-editor/transition-editor.component';
import { NodeEditorComponent } from './workflow-details/node-editor/node-editor.component';
import { LoginComponent } from './login/login.component';
import { TokenInterceptor } from './services/token.interceptor';
import { NavigationPathComponent } from './navigation-path/navigation-path.component';
import { ScheduledTasksComponent } from './scheduled-tasks/scheduled-tasks.component';
import { ScheduledTaskEditorComponent } from './scheduled-task-editor/scheduled-task-editor.component';
import { DisplayPipe } from './pipes/display.pipe';
import { ObjectStructurePipe } from './pipes/object-structure.pipe';
import { ConfirmDeleteComponent } from './dialogs/confirm-delete/confirm-delete.component';
import { TestWorkableComponent } from './dialogs/test-workable/test-workable.component';

@NgModule({
  declarations: [
    AppComponent,
    TasksComponent,
    TaskDetailsComponent,
    ScriptsComponent,
    ScriptDetailsComponent,
    WorkflowsComponent,
    WorkflowDetailsComponent,
    TransitionEditorComponent,
    NodeEditorComponent,
    LoginComponent,
    NavigationPathComponent,
    ScheduledTasksComponent,
    ScheduledTaskEditorComponent,
    DisplayPipe,
    ObjectStructurePipe,
    ConfirmDeleteComponent,
    TestWorkableComponent,
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    HttpClientModule,
    MatToolbarModule,
    MatSidenavModule,
    MatListModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatGridListModule,
    MatInputModule,
    MatCheckboxModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatTableModule,
    MatPaginatorModule,
    MatSnackBarModule,
    FormsModule,
    CommonModule,
    NgxGraphModule,
    NgxChartsModule,
    NgxSelectModule,
    TooltipModule,
    MatMenuModule,
    MatDialogModule,
    ReactiveFormsModule,
    NgbModule,
    MonacoEditorModule.forRoot({baseUrl: "./assets"}),
  ],
  entryComponents: [
    TransitionEditorComponent,
    NodeEditorComponent,
    ConfirmDeleteComponent,
    TestWorkableComponent
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: TokenInterceptor, multi: true},
    { provide: MAT_SNACK_BAR_DEFAULT_OPTIONS, useValue: {duration: 5000}}
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
