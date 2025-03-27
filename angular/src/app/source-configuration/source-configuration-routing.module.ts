import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SourceConfigurationComponent } from './source-configuration.component';

const routes: Routes = [
    {
      path: "",
      component: SourceConfigurationComponent,
    },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SourceConfigurationRoutingModule { }
