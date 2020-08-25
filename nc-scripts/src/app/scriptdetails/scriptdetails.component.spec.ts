import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ScriptDetailsComponent } from './scriptdetails.component';

describe('ScriptDetailsComponent', () => {
  let component: ScriptDetailsComponent;
  let fixture: ComponentFixture<ScriptDetailsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ScriptDetailsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ScriptDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
