import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Horaire } from './horaire.component';

describe('Horaire', () => {
  let component: Horaire;
  let fixture: ComponentFixture<Horaire>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [Horaire]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Horaire);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
