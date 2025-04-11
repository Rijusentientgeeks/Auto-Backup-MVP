import { Injectable, Injector } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MessageService } from '@node_modules/abp-ng2-module';

@Injectable({providedIn: 'root'})
export class VoiceService {
  recognition: any;
  message: MessageService;

  constructor(private http: HttpClient,injector: Injector) {
    const SpeechRecognition = (window as any).webkitSpeechRecognition || (window as any).SpeechRecognition;
    this.recognition = new SpeechRecognition();
    this.recognition.lang = 'en-US';
    this.recognition.continuous = false;
    this.recognition.interimResults = false;
    this.message = injector.get(MessageService);
  }

  startListening() {
    this.recognition.start();
    this.recognition.onresult = (event: any) => {
      const command = event.results[0][0].transcript;
      this.sendCommandToBackend(command);
    };
  }

  sendCommandToBackend(command: string) {
    //this.speechRecognitionService.receiveCommand(this.commandDetails).subscribe();
    this.http.post('https://localhost:44311/api/services/app/SpeechRecognition/ReceiveCommand', { command }).subscribe((res: any) => {
        debugger
        if(!res.result){
            this.message.error("Invaild command.");
        }
    });
  }
}
