import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PagesButton } from './pages-button.component';

describe('PagesButton', () => {
  let component: PagesButton;
  let fixture: ComponentFixture<PagesButton>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [PagesButton]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PagesButton);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
