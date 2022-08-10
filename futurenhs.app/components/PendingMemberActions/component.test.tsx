import { render, screen, cleanup } from "@jestMocks/index";

import { PendingMemberActions } from './index';

import { Props } from "./interfaces";

describe('Pending member actions', () => {

    const props: Props = {
        memberId: '123',
        acceptAction: null,
        rejectAction: null,
        text: {
            acceptMember: 'Accept',
            rejectMember: 'Reject'
        }
    }
    
    it('Renders correctly', () => {

        render(<PendingMemberActions {...props} />)

        expect(screen.getAllByText('Accept').length).toBe(1)
    })

});