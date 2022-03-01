import React from 'react';
import * as nextRouter from 'next/router';
import { render, screen } from '@testing-library/react';

import { createFolderForm } from '@formConfigs/create-folder';
import { GroupCreateFolderTemplate } from './index';
import { Props } from './interfaces';

describe('Group folders template', () => {

    (nextRouter as any).useRouter = jest.fn();
    (nextRouter as any).useRouter.mockImplementation(() => ({ 
        asPath: '/groups/group/files',
        query: {
            groupId: 'group'
        } 
    }));

    const props: Props = {
        id: 'mockId',
        tabId: 'files',
        folderId: 'mockId',
        user: undefined,
        actions: [],
        forms: {
            'create-folder': createFolderForm
        },
        contentText: null,
        entityText: null,
        image: null
    };

    it('renders correctly', () => {

        render(<GroupCreateFolderTemplate {...props} />);

        expect(screen.getAllByText('Save and continue').length).toEqual(1);

    });
    
});
