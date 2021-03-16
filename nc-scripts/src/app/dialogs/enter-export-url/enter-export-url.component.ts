import { Component } from '@angular/core';
import { MatDialogRef } from '@angular/material';

@Component({
  selector: 'app-enter-export-url',
  templateUrl: './enter-export-url.component.html',
  styleUrls: ['./enter-export-url.component.scss']
})
export class EnterExportUrlComponent {
  url: string="";

  constructor(private dialogRef: MatDialogRef<EnterExportUrlComponent>) { }

  close():void {
    this.dialogRef.close(this.url);
  }
}
