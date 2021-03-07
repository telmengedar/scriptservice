import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TestWorkableComponent } from './test-workable.component';

describe('TestWorkableComponent', () => {
  let component: TestWorkableComponent;
  let fixture: ComponentFixture<TestWorkableComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TestWorkableComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TestWorkableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
