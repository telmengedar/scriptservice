import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import {CommonModule} from "@angular/common"

import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppRoutingModule } from './app-routing.module';

import { MatIconModule, MatSidenavModule, MatListModule, MatButtonModule, MatNativeDateModule, MatMenuModule, MatDialogModule} from  '@angular/material';
import {MatToolbarModule} from '@angular/material/toolbar';
import {MatGridListModule} from '@angular/material/grid-list';
import {MatInputModule} from '@angular/material/input';
import {MatCheckboxModule} from '@angular/material/checkbox';
import {MatDatepickerModule} from '@angular/material/datepicker';
import {MatSelectModule} from '@angular/material/select';

import { TasksComponent } from './tasks/tasks.component';
import { TaskDetailsComponent } from './taskdetails/taskdetails.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ScriptsComponent } from './scripts/scripts.component';
import { ScriptDetailsComponent } from './scriptdetails/scriptdetails.component';

import { NgxGraphModule } from '@swimlane/ngx-graph';
import {NgxChartsModule} from '@swimlane/ngx-charts';

import {TooltipModule} from "ngx-tooltip";

import { MonacoEditorModule } from 'ngx-monaco-editor';
import { FormsModule } from '@angular/forms';
import { WorkflowsComponent } from './workflows/workflows.component';
import { WorkflowDetailsComponent } from './workflow-details/workflow-details.component';
import { TransitionEditorComponent } from './workflow-details/transition-editor/transition-editor.component';
import { NodeEditorComponent } from './workflow-details/node-editor/node-editor.component';


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
    NodeEditorComponent
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
    NgbModule,
    MatNativeDateModule,
    FormsModule,
    CommonModule,
    NgxGraphModule,
    NgxChartsModule,
    TooltipModule,
    MatMenuModule,
    MatDialogModule,
    MonacoEditorModule.forRoot()
  ],
  entryComponents: [
    TransitionEditorComponent,
    NodeEditorComponent
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
