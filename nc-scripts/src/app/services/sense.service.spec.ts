import { TestBed } from '@angular/core/testing';

import { SenseService } from './sense.service';

describe('SenseService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: SenseService = TestBed.get(SenseService);
    expect(service).toBeTruthy();
  });
});
