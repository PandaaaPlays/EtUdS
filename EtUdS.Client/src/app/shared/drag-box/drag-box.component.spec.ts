import { ComponentFixture, TestBed } from '@angular/core/testing';
import {DragBox} from './drag-box.component';


describe('DragBoxComponent', () => {
  let component: DragBox;
  let fixture: ComponentFixture<DragBox>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DragBox]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DragBox);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
