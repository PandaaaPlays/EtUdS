import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CoursNoteComponent } from './cours-note.component';

describe('CoursNoteComponent', () => {
  let component: CoursNoteComponent;
  let fixture: ComponentFixture<CoursNoteComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [CoursNoteComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CoursNoteComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
