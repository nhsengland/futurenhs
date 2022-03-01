import React from 'react';
import * as nextRouter from 'next/router';
import { render, screen } from '@testing-library/react';

import { GroupFolderContentsTemplate } from './index';
import { Props } from './interfaces';

describe('Group folders template', () => {

    (nextRouter as any).useRouter = jest.fn();
    (nextRouter as any).useRouter.mockImplementation(() => ({ 
        asPath: '/groups/group/folders',
        query: {
            groupId: 'group',
            folderId: 'folder'
        } 
    }));

    const props: Props = {
        id: 'mockId',
        tabId: 'files',
        folderId: null,
        folder: null,
        folderContents: [],
        user: undefined,
        actions: [],
        contentText: {
            foldersHeading: 'Folders',
            noFolders: 'No folders',
            createFile: 'Create file',
            createFolder: 'Create folder',
            updateFolder: 'Update folder',
            deleteFolder: 'Delet Folder'
        },
        entityText: {
            title: 'Mock title text',
            metaDescription: 'Mock meta description text',
            mainHeading: 'Mock main heading html',
            intro: 'Mock intro html',
            navMenuTitle: 'Mock nav menu title text',
            secondaryHeading: 'Mock secondary heading html'
        },
        image: null
    };

    it('renders correctly', () => {

        render(<GroupFolderContentsTemplate {...props} />);

        expect(screen.getAllByText('Folders').length).toEqual(1);

    });
    
});
