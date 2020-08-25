import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TransitionEditorComponent } from './transition-editor.component';

describe('TransitionEditorComponent', () => {
  let component: TransitionEditorComponent;
  let fixture: ComponentFixture<TransitionEditorComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TransitionEditorComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TransitionEditorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
