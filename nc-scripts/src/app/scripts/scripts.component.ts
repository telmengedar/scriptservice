import { Component, OnInit } from '@angular/core';
import { ScriptService } from '../services/script.service';
import { Script } from '../dto/scripts/script';
import { Router } from '@angular/router';
import { Column } from '../dto/column';
import { Paging } from '../helpers/paging';
import { MatDialog, MatTableDataSource } from '@angular/material';
import { Tables } from '../helpers/tables';
import { ConfirmDeleteComponent } from '../dialogs/confirm-delete/confirm-delete.component';

@Component({
  selector: 'app-scripts',
  templateUrl: './scripts.component.html',
  styleUrls: ['./scripts.component.css']
})
export class ScriptsComponent implements OnInit {
  Paging=Paging;
  Tables=Tables;
  
  scripts: MatTableDataSource<Script>=new MatTableDataSource<Script>();
  
  page: number
  pages: number

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
      display: "Language",
      name: "language",
    },
    {
      display: "",
      name: "_actions",
    }
  ]

  constructor(private scriptservice: ScriptService, private router: Router, private dialog: MatDialog) { }

  ngOnInit() {
    this.loadScripts();
  }

  /**
   * loads current script page
   */
  loadScripts(): void {
    this.scriptservice.listScripts({}).subscribe(s=>{
      this.scripts.data=s.result;
      this.pages=Math.ceil(s.total/50);
      if(s.continue) {
        this.page=Math.floor(s.continue/50)
      }
      else this.page=this.pages;
    });
  }

  /**
   * deletes a script
   * @param script script to delete
   */
  deleteScript(script: Script): void {
    const dialogRef=this.dialog.open(ConfirmDeleteComponent, {
      data: script
    });

    dialogRef.afterClosed().subscribe(r=>{
      if(r)
      {
        this.scriptservice.deleteScript(script.id).subscribe(s=>{
          this.loadScripts();
        });    
      }
    });
  }

  /**
   * changes to the page used to create a new script
   */
  newScript(): void {
    this.router.navigateByUrl(`/scripts/create`);
  }

  /**
   * changes to the page used to edit a script
   * @param scriptId id of script to edit
   */
  editScript(scriptId: number): void {
    this.router.navigateByUrl(`/scripts/${scriptId}`);
  }
}
