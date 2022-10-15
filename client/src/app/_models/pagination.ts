export interface Pagination{
    currentPage: number;
    itemsPerPage: number;
    totalItems: number;
    totalPages: number;
}

export class PagninatedResult<T>{
    result: T; //in our case T is going to be [] of members
    pagination: Pagination;

}