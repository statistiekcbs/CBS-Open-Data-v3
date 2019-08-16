import { Component } from '@angular/core';

import { CbsService } from './cbs.service';

@Component({
  selector: 'my-app',
  templateUrl: './app.component.html'
})
export class AppComponent  {
  name = 'Angular';

  public dataId = '';
  public data: object;
  public hasError = false;

  constructor (private cbsService: CbsService) {}

  public zoek() {
    this.cbsService.getMetaData(this.dataId).subscribe( data => {
      this.hasError = false;
      this.data = data;
      console.log(data);
    }, error => {
      this.hasError = true;
      this.data = undefined;
    });
  }
}
