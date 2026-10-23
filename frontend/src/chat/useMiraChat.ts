import {useRef, useState} from 'react';
import {apiRequest} from '../api/client';

type Message = {id: number; role: 'user' | 'assistant'; text: string};

export default function useMiraChat(token?: string) {
    const [messages, setMessages] = useState<Message[]>([]);
    const [draft, setDraft] = useState('');
    const [error, setError] = useState('');
    const [sending, setSending] = useState(false);
    const generation = useRef(0);
    const pending = useRef(false);
    const nextId = useRef(0);
    const send = async (value = draft) => {
        const message = value.trim();
        if (!message || pending.current) return;
        if (!token) { setError('Please sign in to chat with MIRA.'); return; }
        const current = generation.current;
        pending.current = true;
        setSending(true); setError(''); setDraft('');
        setMessages(previous => [...previous, {id: ++nextId.current, role: 'user', text: message}]);
        try {
            const result = await apiRequest<{message: string}>('/api/mira', {method: 'POST', token, body: {message}});
            if (generation.current === current) setMessages(previous => [...previous, {id: ++nextId.current, role: 'assistant', text: result.message}]);
        } catch (err) {
            if (generation.current === current) setError(err instanceof Error ? err.message : 'MIRA is unavailable. Please try again.');
        } finally {
            if (generation.current === current) { pending.current = false; setSending(false); }
        }
    };
    const clear = () => { generation.current++; pending.current = false; setMessages([]); setDraft(''); setError(''); setSending(false); };
    return {clear, messages, draft, setDraft, sending, send, error};
}
