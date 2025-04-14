import { ComponentFixture, TestBed } from '@angular/core/testing';
import {ConversationButton} from './conversation-button.component';


describe('ChatComponent', () => {
  let component: ConversationButton;
  let fixture: ComponentFixture<ConversationButton>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ConversationButton]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ConversationButton);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
