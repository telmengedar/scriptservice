import { Component, OnInit } from '@angular/core';
import { Script } from '../dto/scripts/script';
import {Location} from '@angular/common';
import { ScriptService } from '../services/script.service';
import { PatchOperation } from '../dto/patchoperation';
import { Patch } from '../helpers/patch';
import { ActivatedRoute } from '@angular/router';
import { ScriptLanguage } from '../dto/scripts/scriptlanguage';
import { ScriptLanguageOptions } from '../dto/scripts/scriptlanguageoptions';
import { MatDialog, MatSnackBar } from '@angular/material';
import { TestWorkableComponent } from '../dialogs/test-workable/test-workable.component';
import { WorkableType } from '../dto/tasks/workabletype';

/**
 * editor component for a single script.
 * Allows to edit an existing script or, when called without a script id, creates a new script on save.
 */
@Component({
  selector: 'app-scriptdetails',
  templateUrl: './scriptdetails.component.html',
  styleUrls: ['./scriptdetails.component.css']
})
export class ScriptDetailsComponent implements OnInit {
  script: Script={
    name: "",
    code: "",
    language: ScriptLanguage.JavaScript
  };

  oldscript: Script;

  languages=ScriptLanguageOptions;

  editorOptions = {
    theme: 'vs-dark', 
    language: 'csharp',
    fontSize: "16px",
    scrollBeyondLastLine: false,
    minimap: {
      enabled: false
    },
    scrollbar: {
      vertical: 'auto',
      horizontal: 'auto'
    },
    highlightActiveIndentGuide: true,
    renderLineHighlight: "gutter"
  };
  changed:boolean=false;

  constructor(private scriptservice: ScriptService, private location: Location, route: ActivatedRoute, private snackbar: MatSnackBar, private dialog: MatDialog) { 
    if(route.snapshot.params.scriptId!=="create") {
      this.script.id=route.snapshot.params.scriptId;
    }
  }

  ngOnInit() {
    if(this.script.id) {
      this.scriptservice.getScript(this.script.id).subscribe(s=>this.scriptLoaded(s));
    }
    else this.oldscript=Object.assign({}, this.script);
  }

  /**
   * navigates back to the script list page
   */
  back(): void {
    this.location.back();
  }

  /**
   * saves the current script
   */
  save(): void {
    if(this.script.id)
    {
      let patches:PatchOperation[]=Patch.generatePatches(this.oldscript, this.script);
      this.scriptservice.patchScript(this.script.id, patches).subscribe(s=>this.scriptLoaded(s));
    }
    else {
      this.scriptservice.createScript(this.script).subscribe(s=>this.scriptLoaded(s));
    }
  }

  /**
   * executes test for the current script with the specified parameters
   */
  startTest(): void {
    this.dialog.open(TestWorkableComponent, {
      data: {
        type: WorkableType.Script,
        workable: this.script
      },
      width: '50%'
    });
  }

  private scriptLoaded(script: Script): void {
    this.script=script;
    this.script.language=ScriptLanguage.getValue(this.script.language);
    this.oldscript=Object.assign({}, this.script);
    this.changed=false;
  }

  /**
   * loads a script revision
   */
  loadRevision(): void {
    if(!this.script.revision)
      return;

    if(this.script.revision<=0)
    {
      this.scriptservice.getScript(this.script.id).subscribe(s=>this.scriptLoaded(s));
      return;
    }
    
    this.scriptservice.getScriptRevision(this.script.id, this.script.revision)
    .toPromise()
    .then(s=>{
      this.script=s;
      this.script.language=ScriptLanguage.getValue(this.script.language);
      this.oldscript={
        id: s.id,
        revision: s.revision,
        name: undefined,
        code: undefined,
        language: undefined
      };

      this.changed=true;
    })
    .catch(e=>{
      this.snackbar.open(e.error.text, "Close");
    });
  }
}
