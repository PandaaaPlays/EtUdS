import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DevoirCard } from './devoir-card.component';

describe('DevoirCard', () => {
  let component: DevoirCard;
  let fixture: ComponentFixture<DevoirCard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DevoirCard]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DevoirCard);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
