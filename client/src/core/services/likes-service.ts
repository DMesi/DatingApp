import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Member } from '../../types/member';
import { PaginatedResult } from '../../types/pagination';

@Injectable({
  providedIn: 'root',
})
export class LikesService {
  
private baseUrl = environment.apiUrl;
private http=inject(HttpClient);

likeIds = signal<string[]>([]);

toggleLike(targetMemberId: string){

return this.http.post(`${this.baseUrl}likes/${targetMemberId}`,{}).subscribe({

next:()=>{

  if(this.likeIds().includes(targetMemberId)){

    this.likeIds.update(ids => ids.filter(x=>x !==targetMemberId));

  }else{

    this.likeIds.update(ids=>[...ids,targetMemberId])
  }
}

})


}

// getLikes(predicate: string){


//   return this.http.get<Member[]>(this.baseUrl+'likes?predicate='+ predicate);

// }


getLikes(predicate:string, pageNumber:number,pageSize:number){

let params = new HttpParams();

params = params.append('pageNumber', pageNumber);

params = params.append('pageSize', pageSize);

params = params.append('predicate', predicate);


return this.http.get<PaginatedResult<Member>>(this.baseUrl +'likes', {params});


// getMembers(memberParams:MemberParams){

// let params = new HttpParams();
// params= params.append('pageNumber',memberParams.pageNumber);
// params= params.append('pageSize',memberParams.pageSize); 
// params= params.append('minAge',memberParams.minAge) ;
// params= params.append('maxAge',memberParams.maxAge);

// params= params.append('orderBy',memberParams.orderBy);


// if(memberParams.gender) params = params.append('gender', memberParams.gender);


// return this.http.get<PaginatedResult<Member>>(this.baseUrl+'members',{params}).pipe(


//   tap(()=>{

//     localStorage.setItem('filters',JSON.stringify(memberParams))
//   }






}

getLikeIds(){

  return this.http.get<string[]>(this.baseUrl+'likes/list').subscribe({

next:ids =>this.likeIds.set(ids)

  })
}

clearLikeIds(){

this.likeIds.set([]);

}


}
