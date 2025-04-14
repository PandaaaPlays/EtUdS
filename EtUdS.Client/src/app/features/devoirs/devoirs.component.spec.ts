import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Devoirs } from './devoirs.component';

describe('Devoirs', () => {
  let component: Devoirs;
  let fixture: ComponentFixture<Devoirs>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [Devoirs]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Devoirs);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
