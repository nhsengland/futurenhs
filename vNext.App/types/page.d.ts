import { actions } from '@constants/actions';
import { Image } from './image';

import { Pagination } from '@appTypes/pagination';
import { FormConfig } from '@appTypes/form';
import { GenericPageTextContent, GroupsPageTextContent } from '@appTypes/content';
import { User } from '@appTypes/user';

export interface Page {
    id: string;
    groupId?: string;
    csrfToken?: string;
    forms?: Record<string, FormConfig>;
    pagination?: Pagination;
    errors?: Array<Record<string>>;
    contentText?: GenericPageTextContent;
    user?: User;
}

export interface GroupPage extends Page {
    image: Image;
    contentText: GroupsPageTextContent;
    entityText: any;
    actions: Array<actions>;
}
