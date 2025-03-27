import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { ManageStorageComponent } from "./manage-storage.component";

const routes: Routes = [
  {
    path: "",
    component: ManageStorageComponent,
  },
  { path: '', redirectTo: '/manage-storage', pathMatch: 'full' },

];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ManageStorageRoutingModule {}
