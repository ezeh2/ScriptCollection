import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChangeList } from './change-list';

describe('ChangeList', () => {
  let component: ChangeList;
  let fixture: ComponentFixture<ChangeList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ChangeList]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ChangeList);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
