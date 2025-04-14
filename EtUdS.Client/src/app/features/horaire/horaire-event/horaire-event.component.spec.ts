import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HoraireEvent } from './horaire-event.component';

describe('HoraireEvent', () => {
  let component: HoraireEvent;
  let fixture: ComponentFixture<HoraireEvent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [HoraireEvent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(HoraireEvent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
