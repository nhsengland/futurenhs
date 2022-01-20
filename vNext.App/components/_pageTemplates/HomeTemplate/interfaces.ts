import { Page } from '@appTypes/page';
import { GenericPageTextContent } from '@appTypes/content';

export interface Props extends Page {
    text: GenericPageTextContent;
}