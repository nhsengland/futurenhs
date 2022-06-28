export interface CmsContentPage {
    item: {
        id: string;
        contentType: string;
        editedAt: string;
        name?: string;
    };
    content: {
        blocks: Array<CmsContentBlock>;
    };
}

export interface CmsContentBlock {
    item: {
        id: string;
        contentType: string;
        name?: string;
    };
    content: Record<any, any>;
}
