// TODO

import { routeParams } from "@constants/routes";
import { ContentType } from "./search-content";

export interface SearchResult {
   type: ContentType;
   entityIds: Record<routeParams,string>;
   meta?: SearchResult;
   content?: {
      title: string;
      body: string;
   }
}
