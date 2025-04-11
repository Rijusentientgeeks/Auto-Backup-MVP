import { Component, ChangeDetectionStrategy } from '@angular/core';
import { VoiceService } from '@shared/voice/voice.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class HeaderComponent {
  constructor(
      private voiceService: VoiceService
    ) {}

    startVoice() {
      this.voiceService.startListening();
    }
}
