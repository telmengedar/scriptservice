import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { NavigationPathComponent } from './navigation-path.component';

describe('NavigationPathComponent', () => {
  let component: NavigationPathComponent;
  let fixture: ComponentFixture<NavigationPathComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ NavigationPathComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(NavigationPathComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
