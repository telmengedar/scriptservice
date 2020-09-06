import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ScheduledTaskEditorComponent } from './scheduled-task-editor.component';

describe('ScheduledTaskEditorComponent', () => {
  let component: ScheduledTaskEditorComponent;
  let fixture: ComponentFixture<ScheduledTaskEditorComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ScheduledTaskEditorComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ScheduledTaskEditorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
