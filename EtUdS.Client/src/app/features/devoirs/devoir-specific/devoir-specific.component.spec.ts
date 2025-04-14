import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DevoirSpecific } from './devoir-specific.component';

describe('DevoirSpecific', () => {
  let component: DevoirSpecific;
  let fixture: ComponentFixture<DevoirSpecific>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DevoirSpecific]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DevoirSpecific);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
