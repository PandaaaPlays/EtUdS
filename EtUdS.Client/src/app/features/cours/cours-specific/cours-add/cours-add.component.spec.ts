import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CoursAdd } from './cours-add.component';

describe('CoursAdd', () => {
  let component: CoursAdd;
  let fixture: ComponentFixture<CoursAdd>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [CoursAdd]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CoursAdd);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
