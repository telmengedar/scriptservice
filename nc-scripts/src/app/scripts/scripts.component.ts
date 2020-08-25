import { Component, OnInit } from '@angular/core';
import { ScriptService } from '../services/script.service';
import { Script } from '../dto/script';
import { Router } from '@angular/router';

@Component({
  selector: 'app-scripts',
  templateUrl: './scripts.component.html',
  styleUrls: ['./scripts.component.css']
})
export class ScriptsComponent implements OnInit {
  scripts: Script[]
  page: number
  pages: number

  
  constructor(private scriptservice: ScriptService, private router: Router) { }

  ngOnInit() {
    this.loadScripts();
  }

  /**
   * loads current script page
   */
  loadScripts(): void {
    this.scriptservice.listScripts({}).subscribe(s=>{
      this.scripts=s.result;
      this.pages=Math.ceil(s.total/50);
      if(s.continue) {
        this.page=Math.floor(s.continue/50)
      }
      else this.page=this.pages;
    });
  }

  /**
   * deletes a script
   * @param scriptId id of script to delete
   */
  deleteScript(scriptId: number): void {
    this.scriptservice.deleteScript(scriptId).subscribe(s=>{
      this.loadScripts();
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
