import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CommitList } from './commit-list';

describe('CommitList', () => {
  let component: CommitList;
  let fixture: ComponentFixture<CommitList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CommitList]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CommitList);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
