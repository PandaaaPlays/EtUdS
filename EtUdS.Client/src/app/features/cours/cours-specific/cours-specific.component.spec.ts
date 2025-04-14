import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CoursSpecificComponent } from './cours-specific.component';

describe('CoursSpecificComponent', () => {
  let component: CoursSpecificComponent;
  let fixture: ComponentFixture<CoursSpecificComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [CoursSpecificComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CoursSpecificComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
