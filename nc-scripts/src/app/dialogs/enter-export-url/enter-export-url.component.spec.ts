import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EnterExportUrlComponent } from './enter-export-url.component';

describe('EnterExportUrlComponent', () => {
  let component: EnterExportUrlComponent;
  let fixture: ComponentFixture<EnterExportUrlComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EnterExportUrlComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EnterExportUrlComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
