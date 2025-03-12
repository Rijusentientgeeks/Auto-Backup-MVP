import {
  Component,
  OnInit,
  ViewEncapsulation,
  Injector,
  Renderer2
} from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';

@Component({
  templateUrl: './account.component.html',
  encapsulation: ViewEncapsulation.None
})
export class AccountComponent extends AppComponentBase implements OnInit {
  constructor(injector: Injector, private renderer: Renderer2) {
    debugger
    super(injector);
  }

  showTenantChange(): boolean {
    debugger
    return abp.multiTenancy.isEnabled;
  }

  ngOnInit(): void {
    debugger
    this.renderer.addClass(document.body, 'login-page');
  }
}
