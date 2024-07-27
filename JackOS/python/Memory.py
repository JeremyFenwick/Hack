import numpy

#
class Memory:
    ram: []
    freelist: int

    def __init__(self, length):
        self.ram = numpy.zeros(length, dtype=int)
        self.ram[0] = 0
        self.ram[1] = length - 2
        self.freelist = 0
        self.total_free_memory = length - 2

    def alloc(self, size):
        # Traverse the list
        candidate_node = self.freelist
        # If the node will fit
        while True:
            attempted_defragmentation = False
            # If a candidate was found
            if self.ram[candidate_node + 1] > size + 4:
                destination = self.ram[candidate_node]
                previous_size = self.ram[candidate_node + 1]

                # Populate the deliverable node
                self.ram[candidate_node] = -1
                self.ram[candidate_node + 1] = size

                self.ram[candidate_node + size + 2] = destination
                self.ram[candidate_node + size + 3] = previous_size - size - 2

                self.freelist = candidate_node + 2 + size

                return candidate_node + 2

            if self.ram[candidate_node] == 0 and not (attempted_defragmentation):
                self.defragment()
                attempted_defragmentation = True
                continue

            # Candidate was not found and there are no nodes left
            if self.ram[candidate_node] == 0 and (attempted_defragmentation):
                return -1

            # Continue to search
            else:
                candidate_node = self.ram[candidate_node + 1]

    def free(self, node_address):
        previous_starting_node = self.freelist
        self.ram[node_address - 2] = previous_starting_node
        self.freelist = node_address - 2

    def linearize(self):
        working_node = 0

        while True:
            current_node_dest = self.ram[working_node]
            current_node_length = self.ram[working_node + 1]

            # If we are at the end of the memory bank, break
            if working_node + current_node_length + 2 >= self.total_free_memory:
                break

            # If we are not looking at a free memory block, continue
            if current_node_dest == -1:
                working_node = current_node_length + 2
                continue
            else:
                free_node = self.next_free_node(working_node + current_node_length + 2)
                if free_node == -1:
                    break
                self.ram[working_node] = free_node
                working_node = free_node

    def next_free_node(self, start):
        working_node = start
        while True:
            current_node_dest = self.ram[working_node]
            current_node_length = self.ram[working_node + 1]

            if working_node + current_node_length > self.total_free_memory:
                return -1

            if current_node_dest != -1:
                return working_node

            else:
                working_node = working_node + current_node_length + 2

    def defragment(self):
        self.linearize()
        working_node = 0
        while True:
            current_node = self.ram[working_node]
            current_node_length = self.ram[working_node + 1]

            # Hard reset of memory
            if current_node_length >= self.total_free_memory:
                self.ram[0] = 0
                self.ram[1] = 40
                self.freelist = 0
                break

            # If we are at the end of the memory bank, break
            if working_node + current_node_length >= self.total_free_memory:
                break

            next_node = self.ram[current_node_length + 2]
            next_node_length = self.ram[current_node_length + 3]

            if current_node != -1 and next_node != -1:
                self.ram[working_node] = next_node
                self.ram[working_node + 1] = current_node_length + next_node_length + 2
                self.ram[current_node_length + 2] = 0
                self.ram[current_node_length + 3] = 0

            else:
                working_node = working_node + current_node_length + 2
                continue

mem = Memory(20)
result = mem.alloc(2)
mem.free(result)
mem.alloc(3)