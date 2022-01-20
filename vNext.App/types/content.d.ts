export interface GenericPageTextContent {
    metaDescription: string;
    title: string;
    mainHeading: string;
}

export interface GroupsPageTextContent extends GenericPageTextContent {
    intro?: string;
    navMenuTitle?: string;
    secondaryHeading?: string;
    strapLine?: string;
}
