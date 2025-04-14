import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CoursDevoirComponent } from './cours-devoir.component';

describe('CoursDevoirComponent', () => {
  let component: CoursDevoirComponent;
  let fixture: ComponentFixture<CoursDevoirComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [CoursDevoirComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CoursDevoirComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
