import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

const baseUrl = "https://opendata.cbs.nl/ODataApi/odata/";

@Injectable()
export class CbsService {

  constructor(private http: HttpClient) { }

  public getMetaData(id: string) {
    return this.http.get(baseUrl + id);
  }
}
