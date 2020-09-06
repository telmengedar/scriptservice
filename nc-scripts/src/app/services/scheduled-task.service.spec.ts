import { TestBed } from '@angular/core/testing';

import { ScheduledTaskService } from './scheduled-task.service';

describe('ScheduledTaskService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: ScheduledTaskService = TestBed.get(ScheduledTaskService);
    expect(service).toBeTruthy();
  });
});
