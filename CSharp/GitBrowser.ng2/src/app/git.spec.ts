import { TestBed } from '@angular/core/testing';

import { Git } from './git';

describe('Git', () => {
  let service: Git;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Git);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
