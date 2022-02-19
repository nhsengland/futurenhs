import { actions } from '@constants/actions';
import { Image } from './image';

import { Pagination } from '@appTypes/pagination';
import { FormConfig } from '@appTypes/form';
import { Service } from '@appTypes/service';
import { GenericPageTextContent, GroupsPageTextContent } from '@appTypes/content';
import { User } from '@appTypes/user';
import { Theme } from '@appTypes/theme';

export interface Page {
    id: string;
    groupId?: string;
    csrfToken?: string;
    forms?: Record<string, FormConfig>;
    services?: Record<string, Service>;
    pagination?: Pagination;
    errors?: Array<Record<string>>;
    contentText?: GenericPageTextContent;
    user?: User;
    theme?: Theme;
}

export interface GroupPage extends Page {
    image: Image;
    contentText: GroupsPageTextContent;
    entityText: any;
    actions: Array<actions>;
}
