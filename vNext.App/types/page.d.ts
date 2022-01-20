import { actions } from '@constants/actions';
import { Image } from './image';

import { Pagination } from '@appTypes/pagination';
import { GenericPageTextContent, GroupsPageTextContent } from '@appTypes/content';
import { User } from '@appTypes/user';

export interface Page {
    id: string;
    groupId?: string;
    csrfToken?: string;
    pagination?: Pagination;
    errors?: Record<string>;
    text?: GenericPageTextContent;
    user?: User;
}

export interface GroupPage extends Page {
    image: Image;
    text: GroupsPageTextContent;
    actions: Array<actions>;
}
