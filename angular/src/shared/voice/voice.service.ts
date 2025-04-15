import { Injectable, Injector } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MessageService } from '@node_modules/abp-ng2-module';
import { CommandResultDto } from '@shared/service-proxies/service-proxies';
import { AppConsts } from '@shared/AppConsts';
import Swal from 'sweetalert2';

@Injectable({ providedIn: 'root' })
export class VoiceService {
  recognition: any;
  message: MessageService;
  commandResultDetails = new CommandResultDto();

  constructor(private http: HttpClient, injector: Injector) {
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
    this.http.post<CommandResultDto>(AppConsts.remoteServiceBaseUrl + '/api/services/app/SpeechRecognition/ReceiveCommand', { command }).subscribe({
      next: (res: any) => {
        //this.isSaving = false;
        const commandResult = res.result;
        this.commandResultDetails = new CommandResultDto();
        this.commandResultDetails.isValidCommand = commandResult.isValidCommand;
        this.commandResultDetails.result = commandResult.result;
        if (this.commandResultDetails.isValidCommand) {
          Swal.fire("Success", `${this.commandResultDetails.result}`, "success");
        } else {
          Swal.fire("Error", `${this.commandResultDetails.result}`, "error");
        }
      },
      error: () => {
        Swal.fire("Error", "Something went wrong!", "error");
      },
      complete: () => {

      }
    });
  }
}
