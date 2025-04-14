import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Inscription } from './inscription.component';

describe('Inscription', () => {
  let component: Inscription;
  let fixture: ComponentFixture<Inscription>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [Inscription]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Inscription);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
