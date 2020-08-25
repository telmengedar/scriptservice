import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material';
import {MAT_DIALOG_DATA} from '@angular/material/dialog';
import { Transition } from 'src/app/dto/workflows/transition';

@Component({
  selector: 'app-transition-editor',
  templateUrl: './transition-editor.component.html',
  styleUrls: ['./transition-editor.component.css']
})
export class TransitionEditorComponent implements OnInit {

  constructor(private dialog: MatDialogRef<TransitionEditorComponent>, @Inject(MAT_DIALOG_DATA)private transition: Transition) { 
  }

  ngOnInit() {
  }


}
